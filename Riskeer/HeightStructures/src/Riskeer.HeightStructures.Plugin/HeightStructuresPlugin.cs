// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Helpers;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.ExportInfos;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.ImportInfos;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TreeNodeInfos;
using Riskeer.Common.Forms.UpdateInfos;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.Structures;
using Riskeer.Common.Service;
using Riskeer.Common.Util;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Forms.PresentationObjects;
using Riskeer.HeightStructures.Forms.PropertyClasses;
using Riskeer.HeightStructures.Forms.Views;
using Riskeer.HeightStructures.IO;
using Riskeer.HeightStructures.IO.Configurations;
using Riskeer.HeightStructures.Plugin.FileImporters;
using Riskeer.HeightStructures.Service;
using Riskeer.HeightStructures.Util;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.HeightStructures.Plugin
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
                CreateInstance = context => new HeightStructuresFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<HeightStructure, HeightStructureProperties>();
            yield return new PropertyInfo<HeightStructuresContext, StructureCollectionProperties<HeightStructure>>
            {
                CreateInstance = context => new StructureCollectionProperties<HeightStructure>(context.WrappedData)
            };
            yield return new PropertyInfo<HeightStructuresInputContext, HeightStructuresInputContextProperties>
            {
                CreateInstance = context => new HeightStructuresInputContextProperties(
                    context,
                    new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };
            yield return new PropertyInfo<HeightStructuresOutputContext, HeightStructuresOutputProperties>
            {
                CreateInstance = context => new HeightStructuresOutputProperties(context.WrappedData.Output,
                                                                                 context.FailureMechanism,
                                                                                 context.AssessmentSection)
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<HeightStructuresContext>
            {
                Name = RiskeerCommonFormsResources.StructuresImporter_DisplayName,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = RiskeerCommonFormsResources.StructuresIcon,
                IsEnabled = context => context.AssessmentSection.ReferenceLine.Points.Any(),
                FileFilterGenerator = CreateHeightStructureFileFilter(),
                CreateFileImporter = (context, filePath) => CreateHeightStructuresImporter(
                    context, filePath, new ImportMessageProvider(), new HeightStructureReplaceDataStrategy(context.FailureMechanism)),
                VerifyUpdates = context =>
                    VerifyStructuresShouldUpdate(
                        context.FailureMechanism,
                        RiskeerCommonIOResources.VerifyStructuresShouldUpdate_When_importing_Calculation_with_Structure_data_output_will_be_cleared_confirm)
            };

            yield return RiskeerImportInfoFactory.CreateCalculationConfigurationImportInfo<HeightStructuresCalculationGroupContext>(
                (context, filePath) => new HeightStructuresCalculationConfigurationImporter(
                    filePath,
                    context.WrappedData,
                    context.AssessmentSection.HydraulicBoundaryDatabase.Locations,
                    context.AvailableForeshoreProfiles,
                    context.AvailableStructures,
                    context.FailureMechanism));
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return new UpdateInfo<HeightStructuresContext>
            {
                Name = RiskeerCommonDataResources.StructureCollection_TypeDescriptor,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = RiskeerCommonFormsResources.StructuresIcon,
                IsEnabled = context => context.WrappedData.SourcePath != null,
                FileFilterGenerator = CreateHeightStructureFileFilter(),
                CreateFileImporter = (context, filePath) => CreateHeightStructuresImporter(
                    context, filePath, new UpdateMessageProvider(), new HeightStructureUpdateDataStrategy(context.FailureMechanism)),
                CurrentPath = context => context.WrappedData.SourcePath,
                VerifyUpdates = context =>
                    VerifyStructuresShouldUpdate(
                        context.FailureMechanism,
                        RiskeerCommonIOResources.VerifyStructuresShouldUpdate_When_updating_Calculation_with_Structure_data_output_will_be_cleared_confirm)
            };

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                HeightStructuresFailureMechanismSectionsContext, HeightStructuresFailureMechanism, HeightStructuresFailureMechanismSectionResult>(
                new HeightStructuresFailureMechanismSectionResultUpdateStrategy());
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return RiskeerExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<HeightStructuresCalculationGroupContext>(
                (context, filePath) => new HeightStructuresCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any(),
                GetInquiryHelper());

            yield return RiskeerExportInfoFactory.CreateCalculationConfigurationExportInfo<HeightStructuresCalculationScenarioContext>(
                (context, filePath) => new HeightStructuresCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                GetInquiryHelper());
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<HeightStructuresFailureMechanismContext, HeightStructuresFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.CalculationIcon,
                CloseForData = CloseHeightStructuresFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = context => new HeightStructuresFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<
                HeightStructuresScenariosContext,
                CalculationGroup,
                HeightStructuresScenariosView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Scenarios_DisplayName,
                AfterCreate = (view, context) => view.FailureMechanism = context.ParentFailureMechanism,
                CloseForData = CloseScenariosViewForData,
                Image = RiskeerCommonFormsResources.ScenariosIcon
            };

            yield return new ViewInfo<
                ProbabilityFailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>,
                IObservableEnumerable<HeightStructuresFailureMechanismSectionResult>,
                HeightStructuresFailureMechanismResultView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new HeightStructuresFailureMechanismResultView(
                    context.WrappedData,
                    (HeightStructuresFailureMechanism) context.FailureMechanism,
                    context.AssessmentSection)
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<HeightStructuresFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<HeightStructuresCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<HeightStructuresCalculationScenarioContext>(
                CalculationContextChildNodeObjects,
                CalculationContextContextMenuStrip,
                CalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<HeightStructuresInputContext>
            {
                Text = inputContext => RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                Image = inputContext => RiskeerCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<HeightStructuresContext>
            {
                Text = context => RiskeerCommonFormsResources.StructuresCollection_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ForeColor = context => context.WrappedData.Any()
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
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

            yield return new TreeNodeInfo<HeightStructure>
            {
                Text = structure => structure.Name,
                Image = structure => RiskeerCommonFormsResources.StructuresIcon,
                ContextMenuStrip = (structure, parentData, treeViewControl) => Gui.Get(structure, treeViewControl)
                                                                                  .AddPropertiesItem()
                                                                                  .Build()
            };

            yield return new TreeNodeInfo<HeightStructuresScenariosContext>
            {
                Text = context => RiskeerCommonFormsResources.Scenarios_DisplayName,
                Image = context => RiskeerCommonFormsResources.ScenariosIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<ProbabilityFailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };
        }

        #region ViewInfos

        private static bool CloseHeightStructuresFailureMechanismViewForData(HeightStructuresFailureMechanismView view, object data)
        {
            var assessmentSection = data as IAssessmentSection;
            var failureMechanism = data as HeightStructuresFailureMechanism;

            return assessmentSection != null
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, failureMechanism);
        }

        private static bool CloseScenariosViewForData(HeightStructuresScenariosView view, object removedData)
        {
            var failureMechanism = removedData as HeightStructuresFailureMechanism;

            if (removedData is HeightStructuresFailureMechanismContext failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            if (removedData is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<HeightStructuresFailureMechanism>()
                                                    .FirstOrDefault();
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

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
                       .Any(fm => ReferenceEquals(view.FailureMechanism.SectionResults, fm.SectionResults));
            }

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        #endregion

        #region TreeNodeInfos

        #region HeightStructuresFailureMechanismContext TreeNodeInfo

        private static object[] FailureMechanismEnabledChildNodeObjects(HeightStructuresFailureMechanismContext context)
        {
            HeightStructuresFailureMechanism wrappedData = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, assessmentSection), TreeFolderCategory.Input),
                new HeightStructuresCalculationGroupContext(wrappedData.CalculationsGroup, null, wrappedData, assessmentSection),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData, assessmentSection), TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetInputs(HeightStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new HeightStructuresFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                new HeightStructuresContext(failureMechanism.HeightStructures, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetOutputs(HeightStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismAssemblyCategoriesContext(failureMechanism, assessmentSection, () => failureMechanism.GeneralInput.N),
                new HeightStructuresScenariosContext(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection),
                new ProbabilityFailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism, assessmentSection),
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
            IEnumerable<StructuresCalculation<HeightStructuresInput>> calculations = context.WrappedData
                                                                                            .Calculations
                                                                                            .Cast<StructuresCalculation<HeightStructuresInput>>();

            IInquiryHelper inquiryHelper = GetInquiryHelper();

            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(context, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddValidateAllCalculationsInFailureMechanismItem(
                              context,
                              ValidateAllInFailureMechanism,
                              EnableValidateAndCalculateMenuItemForFailureMechanism)
                          .AddPerformAllCalculationsInFailureMechanismItem(
                              context,
                              CalculateAllInFailureMechanism,
                              EnableValidateAndCalculateMenuItemForFailureMechanism)
                          .AddSeparator()
                          .AddClearAllCalculationOutputInFailureMechanismItem(context.WrappedData)
                          .AddClearIllustrationPointsOfCalculationsInFailureMechanismItem(() => IllustrationPointsHelper.HasIllustrationPoints(calculations),
                                                                                          CreateChangeHandler(inquiryHelper, calculations))
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(HeightStructuresFailureMechanismContext context,
                                                                          object parentData,
                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(context, RemoveAllViewsForItem)
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

        private static string EnableValidateAndCalculateMenuItemForFailureMechanism(HeightStructuresFailureMechanismContext context)
        {
            return EnableValidateAndCalculateMenuItem(context.Parent);
        }

        private static void ValidateAllInFailureMechanism(HeightStructuresFailureMechanismContext context)
        {
            ValidateAll(context.WrappedData.Calculations.OfType<StructuresCalculation<HeightStructuresInput>>(),
                        context.Parent);
        }

        private void CalculateAllInFailureMechanism(HeightStructuresFailureMechanismContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                HeightStructuresCalculationActivityFactory.CreateCalculationActivities(context.WrappedData, context.Parent));
        }

        #endregion

        #region HeightStructuresCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(HeightStructuresCalculationGroupContext context)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase calculationItem in context.WrappedData.Children)
            {
                if (calculationItem is StructuresCalculationScenario<HeightStructuresInput> calculation)
                {
                    childNodeObjects.Add(new HeightStructuresCalculationScenarioContext(calculation,
                                                                                        context.WrappedData,
                                                                                        context.FailureMechanism,
                                                                                        context.AssessmentSection));
                }
                else if (calculationItem is CalculationGroup group)
                {
                    childNodeObjects.Add(new HeightStructuresCalculationGroupContext(group,
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

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(HeightStructuresCalculationGroupContext context,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            CalculationGroup group = context.WrappedData;
            IInquiryHelper inquiryHelper = GetInquiryHelper();
            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));
            bool isNestedGroup = parentData is HeightStructuresCalculationGroupContext;

            StructuresCalculation<HeightStructuresInput>[] calculations = group
                                                                          .GetCalculations()
                                                                          .OfType<StructuresCalculation<HeightStructuresInput>>()
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
                builder.AddCustomItem(CreateGenerateHeightStructuresCalculationsItem(context))
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(context, AddCalculation)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem();
            }

            builder.AddUpdateForeshoreProfileOfCalculationsItem(calculations, inquiryHelper,
                                                                SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput)
                   .AddCustomItem(CreateUpdateAllStructuresItem(calculations))
                   .AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(
                       context,
                       ValidateAllInCalculationGroup,
                       EnableValidateAndCalculateMenuItemForCalculationGroup)
                   .AddPerformAllCalculationsInGroupItem(
                       group,
                       context,
                       CalculateAllInCalculationGroup,
                       EnableValidateAndCalculateMenuItemForCalculationGroup)
                   .AddSeparator()
                   .AddClearAllCalculationOutputInGroupItem(group)
                   .AddClearIllustrationPointsOfCalculationsInGroupItem(() => IllustrationPointsHelper.HasIllustrationPoints(calculations),
                                                                        CreateChangeHandler(inquiryHelper, calculations));

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

        private StrictContextMenuItem CreateUpdateAllStructuresItem(
            IEnumerable<StructuresCalculation<HeightStructuresInput>> calculations)
        {
            var contextMenuEnabled = true;
            string toolTipMessage = RiskeerCommonFormsResources.StructuresPlugin_CreateUpdateStructureItem_Update_all_calculations_with_Structure_Tooltip;

            StructuresCalculation<HeightStructuresInput>[] calculationsToUpdate = calculations
                                                                                  .Where(calc => calc.InputParameters.Structure != null
                                                                                                 && !calc.InputParameters.IsStructureInputSynchronized)
                                                                                  .ToArray();

            if (!calculationsToUpdate.Any())
            {
                contextMenuEnabled = false;
                toolTipMessage = RiskeerCommonFormsResources.CreateUpdateContextMenuItem_No_calculations_to_update_ToolTip;
            }

            return new StrictContextMenuItem(RiskeerCommonFormsResources.StructuresPlugin_CreateUpdateStructureItem_Update_all_Structures,
                                             toolTipMessage,
                                             RiskeerCommonFormsResources.UpdateItemIcon,
                                             (o, args) => UpdateStructureDependentDataOfCalculations(calculationsToUpdate))
            {
                Enabled = contextMenuEnabled
            };
        }

        private void UpdateStructureDependentDataOfCalculations(IEnumerable<StructuresCalculation<HeightStructuresInput>> calculations)
        {
            string message = RiskeerCommonFormsResources.VerifyUpdate_Confirm_calculation_outputs_cleared;
            if (StructureDependentDataShouldUpdate(calculations, message))
            {
                foreach (StructuresCalculation<HeightStructuresInput> calculation in calculations)
                {
                    UpdateStructureDerivedCalculationInput(calculation);
                }
            }
        }

        private StrictContextMenuItem CreateGenerateHeightStructuresCalculationsItem(HeightStructuresCalculationGroupContext nodeData)
        {
            StructureCollection<HeightStructure> heightStructures = nodeData.FailureMechanism.HeightStructures;
            bool structuresAvailable = heightStructures.Any();

            string heightStructuresCalculationGroupContextToolTip = structuresAvailable
                                                                        ? RiskeerCommonFormsResources.Generate_Calculations_for_selected_Structures
                                                                        : RiskeerCommonFormsResources.No_Structures_to_generate_Calculations_for;

            return new StrictContextMenuItem(RiskeerCommonFormsResources.CalculationGroup_Generate_calculations,
                                             heightStructuresCalculationGroupContextToolTip,
                                             RiskeerCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) => ShowHeightStructuresSelectionDialog(nodeData))
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

        private static void GenerateHeightStructuresCalculations(HeightStructuresFailureMechanism failureMechanism, IEnumerable<HeightStructure> structures, List<ICalculationBase> calculations)
        {
            foreach (HeightStructure structure in structures)
            {
                var calculation = new StructuresCalculationScenario<HeightStructuresInput>
                {
                    Name = NamingHelper.GetUniqueName(calculations, structure.Name, c => c.Name),
                    InputParameters =
                    {
                        Structure = structure
                    }
                };
                calculations.Add(calculation);
            }

            HeightStructuresHelper.UpdateCalculationToSectionResultAssignments(failureMechanism);
        }

        private static void AddCalculation(HeightStructuresCalculationGroupContext context)
        {
            var calculation = new StructuresCalculationScenario<HeightStructuresInput>
            {
                Name = NamingHelper.GetUniqueName(context.WrappedData.Children, RiskeerCommonDataResources.Calculation_DefaultName, c => c.Name)
            };
            context.WrappedData.Children.Add(calculation);
            context.WrappedData.NotifyObservers();
        }

        private static void CalculationGroupContextOnNodeRemoved(HeightStructuresCalculationGroupContext context, object parentNodeData)
        {
            var parentGroupContext = (HeightStructuresCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);

            HeightStructuresHelper.UpdateCalculationToSectionResultAssignments(context.FailureMechanism);

            parentGroupContext.NotifyObservers();
        }

        private static string EnableValidateAndCalculateMenuItemForCalculationGroup(HeightStructuresCalculationGroupContext context)
        {
            return EnableValidateAndCalculateMenuItem(context.AssessmentSection);
        }

        private static void ValidateAllInCalculationGroup(HeightStructuresCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<StructuresCalculation<HeightStructuresInput>>(), context.AssessmentSection);
        }

        private void CalculateAllInCalculationGroup(CalculationGroup group, HeightStructuresCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                HeightStructuresCalculationActivityFactory.CreateCalculationActivities(group, context.FailureMechanism, context.AssessmentSection));
        }

        #endregion

        #region HeightStructuresCalculationContext TreeNodeInfo

        private static object[] CalculationContextChildNodeObjects(HeightStructuresCalculationScenarioContext context)
        {
            StructuresCalculation<HeightStructuresInput> calculation = context.WrappedData;

            return new object[]
            {
                calculation.Comments,
                new HeightStructuresInputContext(calculation.InputParameters,
                                                 calculation,
                                                 context.FailureMechanism,
                                                 context.AssessmentSection),
                new HeightStructuresOutputContext(calculation, context.FailureMechanism, context.AssessmentSection)
            };
        }

        private ContextMenuStrip CalculationContextContextMenuStrip(HeightStructuresCalculationScenarioContext context,
                                                                    object parentData,
                                                                    TreeViewControl treeViewControl)
        {
            StructuresCalculation<HeightStructuresInput> calculation = context.WrappedData;
            var changeHandler = new ClearIllustrationPointsOfStructuresCalculationHandler(GetInquiryHelper(), calculation);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));
            return builder.AddExportItem()
                          .AddSeparator()
                          .AddDuplicateCalculationItem(calculation, context)
                          .AddSeparator()
                          .AddRenameItem()
                          .AddUpdateForeshoreProfileOfCalculationItem(calculation, GetInquiryHelper(),
                                                                      SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput)
                          .AddCustomItem(CreateUpdateStructureItem(context))
                          .AddSeparator()
                          .AddValidateCalculationItem(
                              context,
                              Validate,
                              EnableValidateAndCalculateMenuItemForCalculation)
                          .AddPerformCalculationItem(
                              calculation,
                              context,
                              Calculate,
                              EnableValidateAndCalculateMenuItemForCalculation)
                          .AddSeparator()
                          .AddClearCalculationOutputItem(calculation)
                          .AddClearIllustrationPointsOfCalculationItem(() => IllustrationPointsHelper.HasIllustrationPoints(calculation), changeHandler)
                          .AddDeleteItem()
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static string EnableValidateAndCalculateMenuItemForCalculation(HeightStructuresCalculationScenarioContext context)
        {
            return EnableValidateAndCalculateMenuItem(context.AssessmentSection);
        }

        private static void Validate(HeightStructuresCalculationScenarioContext context)
        {
            HeightStructuresCalculationService.Validate(context.WrappedData, context.AssessmentSection);
        }

        private void Calculate(StructuresCalculation<HeightStructuresInput> calculation, HeightStructuresCalculationScenarioContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                HeightStructuresCalculationActivityFactory.CreateCalculationActivity(calculation, context.FailureMechanism, context.AssessmentSection));
        }

        private static void CalculationContextOnNodeRemoved(HeightStructuresCalculationScenarioContext context, object parentData)
        {
            if (parentData is HeightStructuresCalculationGroupContext calculationGroupContext)
            {
                calculationGroupContext.WrappedData.Children.Remove(context.WrappedData);
                HeightStructuresHelper.UpdateCalculationToSectionResultAssignments(context.FailureMechanism);
                calculationGroupContext.NotifyObservers();
            }
        }

        private StrictContextMenuItem CreateUpdateStructureItem(HeightStructuresCalculationScenarioContext context)
        {
            var contextMenuEnabled = true;
            string toolTipMessage = RiskeerCommonFormsResources.Update_Calculation_with_Structure_ToolTip;
            if (context.WrappedData.InputParameters.Structure == null)
            {
                contextMenuEnabled = false;
                toolTipMessage = RiskeerCommonFormsResources.Structure_must_be_selected_ToolTip;
            }
            else if (context.WrappedData.InputParameters.IsStructureInputSynchronized)
            {
                contextMenuEnabled = false;
                toolTipMessage = RiskeerCommonFormsResources.CalculationItem_No_changes_to_update_ToolTip;
            }

            return new StrictContextMenuItem(
                RiskeerCommonFormsResources.Update_Structure_data,
                toolTipMessage,
                RiskeerCommonFormsResources.UpdateItemIcon,
                (o, args) => UpdateStructureDependentDataOfCalculation(context.WrappedData))
            {
                Enabled = contextMenuEnabled
            };
        }

        private void UpdateStructureDependentDataOfCalculation(StructuresCalculation<HeightStructuresInput> calculation)
        {
            string message = RiskeerCommonFormsResources.VerifyUpdate_Confirm_calculation_output_cleared;
            if (StructureDependentDataShouldUpdate(new[]
            {
                calculation
            }, message))
            {
                UpdateStructureDerivedCalculationInput(calculation);
            }
        }

        private bool StructureDependentDataShouldUpdate(IEnumerable<StructuresCalculation<HeightStructuresInput>> calculations, string query)
        {
            var changeHandler = new CalculationChangeHandler(calculations, query, GetInquiryHelper());
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        private static void UpdateStructureDerivedCalculationInput(StructuresCalculation<HeightStructuresInput> calculation)
        {
            calculation.InputParameters.SynchronizeStructureInput();

            var affectedObjects = new List<IObservable>
            {
                calculation.InputParameters
            };

            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation));

            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }

        #endregion

        private ClearIllustrationPointsOfStructureCalculationCollectionChangeHandler CreateChangeHandler(
            IInquiryHelper inquiryHelper, IEnumerable<StructuresCalculation<HeightStructuresInput>> calculations)
        {
            return new ClearIllustrationPointsOfStructureCalculationCollectionChangeHandler(inquiryHelper, calculations);
        }

        private static void ValidateAll(IEnumerable<StructuresCalculation<HeightStructuresInput>> heightStructuresCalculations,
                                        IAssessmentSection assessmentSection)
        {
            foreach (StructuresCalculation<HeightStructuresInput> calculation in heightStructuresCalculations)
            {
                HeightStructuresCalculationService.Validate(calculation, assessmentSection);
            }
        }

        private static string EnableValidateAndCalculateMenuItem(IAssessmentSection assessmentSection)
        {
            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        #endregion

        #region ImportInfos

        #region HeightStructuresImporter

        private static IFileImporter CreateHeightStructuresImporter(HeightStructuresContext context, string filePath,
                                                                    IImporterMessageProvider messageProvider, IStructureUpdateStrategy<HeightStructure> strategy)
        {
            return new HeightStructuresImporter(context.WrappedData, context.AssessmentSection.ReferenceLine,
                                                filePath, messageProvider, strategy);
        }

        private static FileFilterGenerator CreateHeightStructureFileFilter()
        {
            return new FileFilterGenerator(RiskeerCommonIOResources.Shape_file_filter_Extension,
                                           RiskeerCommonIOResources.Shape_file_filter_Description);
        }

        private bool VerifyStructuresShouldUpdate(IFailureMechanism failureMechanism, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(failureMechanism, query, GetInquiryHelper());
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #endregion

        #endregion
    }
}