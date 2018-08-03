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
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Ringtoets.Common.Data;
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
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.PropertyClasses;
using Ringtoets.StabilityPointStructures.Forms.Views;
using Ringtoets.StabilityPointStructures.IO;
using Ringtoets.StabilityPointStructures.IO.Configurations;
using Ringtoets.StabilityPointStructures.Plugin.FileImporters;
using Ringtoets.StabilityPointStructures.Service;
using Ringtoets.StabilityPointStructures.Util;
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
                CreateInstance = context => new StabilityPointStructuresFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<StabilityPointStructure, StabilityPointStructureProperties>();
            yield return new PropertyInfo<StabilityPointStructuresInputContext, StabilityPointStructuresInputContextProperties>
            {
                CreateInstance = context => new StabilityPointStructuresInputContextProperties(context, new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };
            yield return new PropertyInfo<StabilityPointStructuresContext, StructureCollectionProperties<StabilityPointStructure>>
            {
                CreateInstance = context => new StructureCollectionProperties<StabilityPointStructure>(context.WrappedData)
            };
            yield return new PropertyInfo<StabilityPointStructuresOutputContext, StabilityPointStructuresOutputProperties>
            {
                CreateInstance = context => new StabilityPointStructuresOutputProperties(context.WrappedData.Output,
                                                                                         context.FailureMechanism,
                                                                                         context.AssessmentSection)
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<StabilityPointStructuresFailureMechanismContext, StabilityPointStructuresFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = CloseStabilityPointStructuresFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = context => new StabilityPointStructuresFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<
                ProbabilityFailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>,
                IObservableEnumerable<StabilityPointStructuresFailureMechanismSectionResult>,
                StabilityPointStructuresFailureMechanismResultView>
            {
                GetViewName = (view, context) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new StabilityPointStructuresFailureMechanismResultView(
                    context.WrappedData,
                    (StabilityPointStructuresFailureMechanism) context.FailureMechanism, context.AssessmentSection)
            };

            yield return new ViewInfo<StabilityPointStructuresScenariosContext, CalculationGroup, StabilityPointStructuresScenariosView>
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
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<StabilityPointStructuresFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return new TreeNodeInfo<ProbabilityFailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>>
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
                                                                                 .AddUpdateItem()
                                                                                 .AddSeparator()
                                                                                 .AddCollapseAllItem()
                                                                                 .AddExpandAllItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<StabilityPointStructure>
            {
                Text = structure => structure.Name,
                Image = structure => RingtoetsCommonFormsResources.StructuresIcon,
                ContextMenuStrip = (structure, parentData, treeViewControl) => Gui.Get(structure, treeViewControl)
                                                                                  .AddPropertiesItem()
                                                                                  .Build()
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
                CreateFileImporter = (context, filePath) => CreateStabilityPointStructuresImporter(
                    context,
                    filePath,
                    new ImportMessageProvider(),
                    new StabilityPointStructureReplaceStrategy(context.FailureMechanism)),
                Name = RingtoetsCommonFormsResources.StructuresImporter_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.StructuresIcon,
                FileFilterGenerator = CreateStabilityPointStructureFileFilter(),
                IsEnabled = context => context.AssessmentSection.ReferenceLine != null,
                VerifyUpdates = context => VerifyStructuresShouldUpdate(
                    context.FailureMechanism,
                    RingtoetsCommonIOResources.VerifyStructuresShouldUpdate_When_importing_Calculation_with_Structure_data_output_will_be_cleared_confirm)
            };

            yield return RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo<StabilityPointStructuresCalculationGroupContext>(
                (context, filePath) => new StabilityPointStructuresCalculationConfigurationImporter(
                    filePath,
                    context.WrappedData,
                    context.AssessmentSection.HydraulicBoundaryDatabase.Locations,
                    context.AvailableForeshoreProfiles,
                    context.AvailableStructures,
                    context.FailureMechanism));
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return RingtoetsExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<StabilityPointStructuresCalculationGroupContext>(
                (context, filePath) => new StabilityPointStructuresCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any());

            yield return RingtoetsExportInfoFactory.CreateCalculationConfigurationExportInfo<StabilityPointStructuresCalculationContext>(
                (context, filePath) => new StabilityPointStructuresCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath));
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return new UpdateInfo<StabilityPointStructuresContext>
            {
                CreateFileImporter = (context, filePath) => CreateStabilityPointStructuresImporter(
                    context,
                    filePath,
                    new UpdateMessageProvider(),
                    new StabilityPointStructureUpdateDataStrategy(context.FailureMechanism)),
                Name = RingtoetsCommonDataResources.StructureCollection_TypeDescriptor,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.StructuresIcon,
                FileFilterGenerator = CreateStabilityPointStructureFileFilter(),
                IsEnabled = context => context.WrappedData.SourcePath != null,
                CurrentPath = context => context.WrappedData.SourcePath,
                VerifyUpdates = context => VerifyStructuresShouldUpdate(
                    context.FailureMechanism,
                    RingtoetsCommonIOResources.VerifyStructuresShouldUpdate_When_updating_Calculation_with_Structure_data_output_will_be_cleared_confirm)
            };

            yield return RingtoetsUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                StabilityPointStructuresFailureMechanismSectionsContext, StabilityPointStructuresFailureMechanism, StabilityPointStructuresFailureMechanismSectionResult>(
                new StabilityPointStructuresFailureMechanismSectionResultUpdateStrategy());
        }

        #region ViewInfo

        #region StabilityPointStructuresFailureMechanismView ViewInfo

        private static bool CloseStabilityPointStructuresFailureMechanismViewForData(StabilityPointStructuresFailureMechanismView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as StabilityPointStructuresFailureMechanism;

            return assessmentSection != null
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, failureMechanism);
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
                       .Any(fm => ReferenceEquals(view.FailureMechanism.SectionResults, fm.SectionResults));
            }

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
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

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection)
        {
            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        private static void ValidateAll(IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations, IAssessmentSection assessmentSection)
        {
            foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in calculations)
            {
                StabilityPointStructuresCalculationService.Validate(calculation, assessmentSection);
            }
        }

        #endregion

        #region TreeNodeInfo

        #region StabilityPointStructuresFailureMechanismContext TreeNodeInfo

        private static object[] FailureMechanismEnabledChildNodeObjects(StabilityPointStructuresFailureMechanismContext context)
        {
            StabilityPointStructuresFailureMechanism wrappedData = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetInputs(wrappedData, assessmentSection),
                                       TreeFolderCategory.Input),
                new StabilityPointStructuresCalculationGroupContext(wrappedData.CalculationsGroup,
                                                                    null,
                                                                    wrappedData,
                                                                    assessmentSection),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetOutputs(wrappedData, assessmentSection),
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

        private static IEnumerable<object> GetInputs(StabilityPointStructuresFailureMechanism failureMechanism,
                                                     IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new StabilityPointStructuresFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                new StabilityPointStructuresContext(failureMechanism.StabilityPointStructures, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetOutputs(StabilityPointStructuresFailureMechanism failureMechanism,
                                                      IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismAssemblyCategoriesContext(failureMechanism, assessmentSection, () => failureMechanism.GeneralInput.N),
                new StabilityPointStructuresScenariosContext(failureMechanism.CalculationsGroup, failureMechanism),
                new ProbabilityFailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism, assessmentSection),
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
            return ValidateAllDataAvailableAndGetErrorMessage(context.Parent);
        }

        private static void ValidateAll(StabilityPointStructuresFailureMechanismContext context)
        {
            ValidateAll(context.WrappedData.Calculations.OfType<StructuresCalculation<StabilityPointStructuresInput>>(),
                        context.Parent);
        }

        private void CalculateAll(StabilityPointStructuresFailureMechanismContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivities(context.WrappedData, context.Parent));
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
                                                                                        context.WrappedData,
                                                                                        context.FailureMechanism,
                                                                                        context.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new StabilityPointStructuresCalculationGroupContext(group,
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

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(StabilityPointStructuresCalculationGroupContext context,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            CalculationGroup group = context.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));
            var inquiryHelper = new DialogBasedInquiryHelper(Gui.MainWindow);
            bool isNestedGroup = parentData is StabilityPointStructuresCalculationGroupContext;

            StructuresCalculation<StabilityPointStructuresInput>[] calculations = group
                                                                                  .GetCalculations()
                                                                                  .OfType<StructuresCalculation<StabilityPointStructuresInput>>()
                                                                                  .ToArray();

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
            IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations)
        {
            var contextMenuEnabled = true;
            string toolTipMessage = RingtoetsCommonFormsResources.StructuresPlugin_CreateUpdateStructureItem_Update_all_calculations_with_Structure_Tooltip;

            StructuresCalculation<StabilityPointStructuresInput>[] calculationsToUpdate = calculations
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

        private void UpdateStructureDependentDataOfCalculations(StructuresCalculation<StabilityPointStructuresInput>[] calculations)
        {
            string message = RingtoetsCommonFormsResources.VerifyUpdate_Confirm_calculation_outputs_cleared;
            if (StructureDependentDataShouldUpdate(calculations, message))
            {
                foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in calculations)
                {
                    UpdateStructureDerivedCalculationInput(calculation);
                }
            }
        }

        private StrictContextMenuItem CreateGenerateStabilityPointStructuresCalculationsItem(StabilityPointStructuresCalculationGroupContext nodeData)
        {
            StructureCollection<StabilityPointStructure> stabilityPointStructures = nodeData.FailureMechanism.StabilityPointStructures;
            bool structuresAvailable = stabilityPointStructures.Any();

            string stabilityPointStructuresCalculationGroupContextToolTip = structuresAvailable
                                                                                ? RingtoetsCommonFormsResources.Generate_Calculations_for_selected_Structures
                                                                                : RingtoetsCommonFormsResources.No_Structures_to_generate_Calculations_for;

            return new StrictContextMenuItem(RingtoetsCommonFormsResources.CalculationGroup_Generate_calculations,
                                             stabilityPointStructuresCalculationGroupContextToolTip,
                                             RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) => ShowStabilityPointStructuresSelectionDialog(nodeData))
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
                        nodeData.FailureMechanism,
                        dialog.SelectedItems.Cast<StabilityPointStructure>(),
                        nodeData.WrappedData.Children);
                    nodeData.NotifyObservers();
                }
            }
        }

        private static void GenerateStabilityPointStructuresCalculations(StabilityPointStructuresFailureMechanism failureMechanism,
                                                                         IEnumerable<StabilityPointStructure> structures,
                                                                         List<ICalculationBase> calculations)
        {
            foreach (StabilityPointStructure structure in structures)
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

            StabilityPointStructuresHelper.UpdateCalculationToSectionResultAssignments(failureMechanism);
        }

        private static void CalculationGroupContextOnNodeRemoved(StabilityPointStructuresCalculationGroupContext context, object parentNodeData)
        {
            var parentGroupContext = (StabilityPointStructuresCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);

            StabilityPointStructuresHelper.UpdateCalculationToSectionResultAssignments(context.FailureMechanism);

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
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection);
        }

        private static void ValidateAll(StabilityPointStructuresCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<StructuresCalculation<StabilityPointStructuresInput>>(),
                        context.AssessmentSection);
        }

        private void CalculateAll(CalculationGroup group, StabilityPointStructuresCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivities(group,
                                                                                                                            context.FailureMechanism,
                                                                                                                            context.AssessmentSection));
        }

        #endregion

        #region StabilityPointStructuresCalculationContext TreeNodeInfo

        private static object[] CalculationContextChildNodeObjects(StabilityPointStructuresCalculationContext context)
        {
            StructuresCalculation<StabilityPointStructuresInput> calculation = context.WrappedData;

            return new object[]
            {
                calculation.Comments,
                new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                         calculation,
                                                         context.FailureMechanism,
                                                         context.AssessmentSection),
                new StabilityPointStructuresOutputContext(calculation, context.FailureMechanism, context.AssessmentSection)
            };
        }

        private ContextMenuStrip CalculationContextContextMenuStrip(StabilityPointStructuresCalculationContext context,
                                                                    object parentData,
                                                                    TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));
            var inquiryHelper = new DialogBasedInquiryHelper(Gui.MainWindow);

            StructuresCalculation<StabilityPointStructuresInput> calculation = context.WrappedData;
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
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection);
        }

        private void Calculate(StructuresCalculation<StabilityPointStructuresInput> calculation, StabilityPointStructuresCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
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
                StabilityPointStructuresHelper.UpdateCalculationToSectionResultAssignments(context.FailureMechanism);
                calculationGroupContext.NotifyObservers();
            }
        }

        private StrictContextMenuItem CreateUpdateStructureItem(StabilityPointStructuresCalculationContext context)
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

        private void UpdateStructureDependentDataOfCalculation(StructuresCalculation<StabilityPointStructuresInput> calculation)
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

        private bool StructureDependentDataShouldUpdate(IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations, string query)
        {
            var changeHandler = new CalculationChangeHandler(calculations,
                                                             query,
                                                             new DialogBasedInquiryHelper(Gui.MainWindow));

            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        private static void UpdateStructureDerivedCalculationInput(ICalculation<StabilityPointStructuresInput> calculation)
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

        private static StabilityPointStructuresImporter CreateStabilityPointStructuresImporter(StabilityPointStructuresContext context,
                                                                                               string filePath,
                                                                                               IImporterMessageProvider messageProvider,
                                                                                               IStructureUpdateStrategy<StabilityPointStructure> updateStrategy)
        {
            return new StabilityPointStructuresImporter(context.WrappedData,
                                                        context.AssessmentSection.ReferenceLine,
                                                        filePath,
                                                        messageProvider,
                                                        updateStrategy);
        }

        private static FileFilterGenerator CreateStabilityPointStructureFileFilter()
        {
            return new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
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
    }
}